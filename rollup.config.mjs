import typescript from '@rollup/plugin-typescript';

export default {
	input: './TypeScripts/main.mts',
	output: {
		file: './dist/bundle.js',
	},
	plugins: [
		typescript({
			outDir: "./dist",
			sourceMap: false,
			declaration: false,
			declarationMap: false,
		})
	]
};