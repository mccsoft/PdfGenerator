const { TsConfigPathsPlugin } = require('awesome-typescript-loader');

module.exports = {
    resolve: {
        extensions: ['.ts', '.tsx', '.js', '.jsx'],
        plugins: [
            new TsConfigPathsPlugin(),
        ]
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                exclude: /node_modules/,
                loader: 'awesome-typescript-loader'
            },
            {
                test: /\.(scss|css)$/,
                exclude: /node_modules/,
                use: [
                    'style-loader',
                    'css-loader',
                    'sass-loader'
                ]
            },
            {
                test: /\.(png|jpg|woff2)$/,
                exclude: /node_modules/,
                loader: 'url-loader'
            }
        ]
    }
};
