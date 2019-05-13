const mdpdf = require('mdpdf');
const path = require('path');

let options = {
	//styles: path.join(__dirname, '/node_modules/github-markdown-css/github-markdown.css'),
	ghStyle : true,
	defaultStyle : true,
	pdf: {
		format: 'A4',
		orientation: 'portrait'
	}
};

const labsFolder = __dirname+'/..';
const fs = require('fs');

fs.readdir(labsFolder, (err, files) => {
	files.forEach(file => {
		if (file.endsWith(".md")) {
			sourcePath = path.join(labsFolder, file);
			console.log(sourcePath);
			fileOptions = {
				...options,
				source: sourcePath,
				destination: path.join(__dirname + "/../publish", file.substr(0, file.lastIndexOf(".")) + ".pdf"),
			}
			mdpdf.convert(fileOptions).then((pdfPath) => {
				console.log('PDF Path:', pdfPath);
			}).catch((err) => {
				console.error(err);
			});
		}
	});

});
