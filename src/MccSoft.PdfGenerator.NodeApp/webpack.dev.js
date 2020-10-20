const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const commonConfig = require('./webpack.common');

module.exports = env => merge(commonConfig, {
    mode: 'development',
    devtool: 'eval-source-map',
    entry: {
        index: path.resolve(__dirname, 'src', 'index.tsx'),
    },
    output: {
        path: path.resolve(__dirname, 'build'),
        filename: 'bundle.js'
    },
    devServer: {
        port: 3100,
        hot: true,
        open: true
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: path.resolve(__dirname, 'public', 'index.html')
        }),
        new webpack.HotModuleReplacementPlugin()
    ]
});
