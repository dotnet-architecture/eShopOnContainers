var path = require('path');
var webpack = require('webpack');
var merge = require('extendify')({ isDeep: true, arrays: 'concat' });
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var extractCSS = new ExtractTextPlugin('styles.css');
var ForkCheckerPlugin = require('awesome-typescript-loader').ForkCheckerPlugin;
var devConfig = require('./webpack.config.dev');
var prodConfig = require('./webpack.config.prod');
var CopyWebpackPlugin = require('copy-webpack-plugin');
var isDevelopment = process.env.ASPNETCORE_ENVIRONMENT === 'Development';

console.log("==========Dev Mode = " + isDevelopment + " ============" )

module.exports = merge({
    resolve: {
        extensions: ['.js', '.ts']
    },
    module: {
        rules: [
            {
                test: /\.ts$/, exclude: [/\.(spec|e2e)\.ts$/],
                loaders: ['awesome-typescript-loader?forkChecker=true ', 'angular2-template-loader', 'angular2-router-loader']
            },
            { test: /\.html$/, loader: "html" },
            { test: /\.scss$/, loader: 'exports-loader?module.exports.toString()!css-loader!sass-loader' },
            { test: /\.json$/, loader: 'json-loader' },
            {
                test: /\.woff(\?v=\d+\.\d+\.\d+)?$/,
                loader: "file-loader"
            }, {
                test: /\.woff2(\?v=\d+\.\d+\.\d+)?$/,
                loader: "file-loader"
            }, {
                test: /\.ttf(\?v=\d+\.\d+\.\d+)?$/,
                loader: "file-loader"
            }, {
                test: /\.eot(\?v=\d+\.\d+\.\d+)?$/,
                loader: "file-loader"
            }, 
            {
                test: /\.(png|jpg|gif|svg)$/,
                loader: "file-loader?name=images/[name].[ext]"
            }
        ]
    },
    entry: {
        'main': './Client/main.ts'
    },
    output: {
        path: path.join(__dirname, '../wwwroot', 'dist'),
        filename: '[name].js',
        publicPath: '/dist/'
    },
    profile: true,
    plugins: [
        extractCSS,
        new webpack.DllReferencePlugin({
            context: __dirname,
            manifest: require('../wwwroot/dist/vendor-manifest.json')
        }),
        // To eliminate warning
        // https://github.com/AngularClass/angular2-webpack-starter/issues/993
        new webpack.ContextReplacementPlugin(
            /angular(\\|\/)core(\\|\/)(esm(\\|\/)src|src)(\\|\/)linker/,
            __dirname
        ),
        new ForkCheckerPlugin(),
        new webpack.DefinePlugin({
            'process.env': {
                'ENV': JSON.stringify(process.env.ASPNETCORE_ENVIRONMENT)
            }
        }),
         new CopyWebpackPlugin([
            { from: 'Client/fonts', to: 'fonts' }
         ])
    ]
}, isDevelopment ? devConfig : prodConfig);
