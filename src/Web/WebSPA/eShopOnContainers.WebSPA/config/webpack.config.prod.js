var webpack = require('webpack');
const WebpackMd5Hash = require('webpack-md5-hash');

module.exports = {
    devtool: 'source-map',
    output: {
        filename: '[name].[chunkhash].bundle.js',
        sourceMapFilename: '[name].[chunkhash].bundle.map',
        chunkFilename: '[id].[chunkhash].chunk.js'
    },
    plugins: [
        // new webpack.LoaderOptionsPlugin({
        //     minimize: true,
        //     debug: false
        // }),
        new WebpackMd5Hash(),
        new webpack.optimize.UglifyJsPlugin({
            beautify: false,
            comments: false,
            sourceMap: true
        })
    ]
};
