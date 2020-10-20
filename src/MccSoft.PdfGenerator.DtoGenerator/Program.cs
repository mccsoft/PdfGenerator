using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MccSoft.PdfGenerator.Dto;
using Reinforced.Typings;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;

namespace MccSoft.PdfGenerator.DtoGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine(
                    "ERROR: No output file specified. Specify a full path as the first argument.");
                Environment.Exit(1);
            }

            string file = args[0];
            ExportContext ec = new ExportContext(sourceAssemblies: new Assembly[0])
            {
                ConfigurationMethod = ConfigureExport,
                Hierarchical = false,
                TargetDirectory = ".",
                TargetFile = file
            };

            TsExporter te = new TsExporter(ec);
            te.Initialize();
            te.Export();

            Console.WriteLine("OK");
        }

        public static void ConfigureExport(ConfigurationBuilder b)
        {
            RtSimpleTypeName str = new RtSimpleTypeName("string");
            b.Substitute(typeof(Guid), str);
            b.Substitute(typeof(DateTimeOffset), str);
            b.Substitute(typeof(DateTime), str);
            b.Substitute(typeof(TimeSpan), str);
            Type[] nested = GetNestedTypes(typeof(ReportDto), new HashSet<Type>()).ToArray();
            b.ExportAsEnums(nested.Where(t => t.IsEnum), c => c.UseString());
            foreach (Type type in nested.Where(t => t.IsClass))
            {
                b.ExportAsInterfaces(new[] {type},
                    x =>
                    {
                        BindingFlags bindingFlags =
                            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
                        IEnumerable<(string, bool)> declaredProps =
                            from PropertyInfo prop in type.GetProperties(bindingFlags)
                            let t = prop.PropertyType
                            let isNullable =
                                !t.IsValueType || Nullable.GetUnderlyingType(t) != null
                            select (prop.Name, isNullable);
                        foreach ((string name, bool isNullable) in declaredProps)
                        {
                            x.WithProperty(name, pb => pb.ForceNullable(isNullable));
                        }
                    });
            }

            b.Global(
                a => a.CamelCaseForProperties()
                    .CamelCaseForMethods()
                    .GenerateDocumentation()
                    .TabSymbol("    ")
                    .UseModules());
        }

        /// <summary>
        /// Traverses the specified type and returns a collection of types that it refers to
        /// from properties or generic arguments, transitively.
        /// System types are excluded.
        /// </summary>
        /// <param name="type">The type to analyze.</param>
        /// <param name="seen">A set of types that have been already analyzed.</param>
        /// <returns>A collection of types the specified type depends on, including itself.</returns>
        public static IEnumerable<Type> GetNestedTypes(Type type, HashSet<Type> seen)
        {
            if (!seen.Add(type))
            {
                yield break;
            }

            if (type.IsGenericType)
            {
                IEnumerable<Type> argTypes =
                    from arg in type.GenericTypeArguments
                    from t in GetNestedTypes(arg, seen)
                    select t;
                foreach (Type nested in argTypes)
                {
                    yield return nested;
                }
            }

            if (type.Namespace.StartsWith("System"))
            {
                yield break;
            }

            yield return type;

            IEnumerable<Type> propTypes =
                from props in type.GetProperties()
                from t in GetNestedTypes(props.PropertyType, seen)
                select t;
            foreach (Type nested in propTypes)
            {
                yield return nested;
            }

            // We have collected types from properties of base types,
            // but not the base types themselves.
            foreach (Type nested in GetNestedTypes(type.BaseType, seen))
            {
                yield return nested;
            }
        }
    }
}