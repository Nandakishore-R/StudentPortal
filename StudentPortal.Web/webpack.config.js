const path = require('path');

module.exports = {
  mode: "development",
  entry: './src/index.js', // Entry point for React components
  output: {
    path: path.resolve(__dirname, 'wwwroot/js'), // Output in the wwwroot folder
    filename: 'bundle.js', // Output file name
  },
  module: {
    rules: [
      {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: ['@babel/preset-env', '@babel/preset-react'],
          },
        },
      },
    ],
  },
  resolve: {
    extensions: ['.js', '.jsx'],
  },
};
