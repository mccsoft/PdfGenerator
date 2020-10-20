export class DevelopmentUtils {
    static isDevelopment: boolean = process.env.NODE_ENV === 'development';

    static initialize(): void {
        if (this.isDevelopment && (module as any).hot) {
            console.clear();
            (module as any).hot.accept();
        }
    }
}
