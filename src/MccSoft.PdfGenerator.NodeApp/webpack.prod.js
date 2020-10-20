const path = require('path');
const merge = require('webpack-merge');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const FileManagerPlugin = require('filemanager-webpack-plugin-fixed');
const commonConfig = require('./webpack.common');

const outputPath = path.resolve(__dirname, 'public', 'bundle');
const templateOutputPath = path.resolve(__dirname, '..', 'MccSoft.PdfGenerator.App', 'Template');

module.exports = merge(commonConfig, {
    mode: 'production',
    entry: {
        'report': path.resolve(__dirname, 'src', 'entries', 'report.tsx')
    },
    output: {
        path: outputPath,
        filename: '[name]-bundle.js'
    },
    plugins: [
        new CleanWebpackPlugin({
                dry: false,
                dangerouslyAllowCleanPatternsOutsideProject: true,
                cleanOnceBeforeBuildPatterns: [
                    outputPath,
                    path.resolve(templateOutputPath, '**', '*'),
                ]
            }
        ),
        new FileManagerPlugin({
            onEnd: {
                copy: [
                    {
                        source: outputPath,
                        destination: templateOutputPath
                    },
                ]
            }
        })
    ],
});
