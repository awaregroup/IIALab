const mdpdf = require('mdpdf');
const path = require('path');

let options = {
	//styles: path.join(__dirname, '/node_modules/github-markdown-css/github-markdown.css'),
	ghStyle : true,
	pdf: {
		format: 'A4',
		orientation: 'portrait',
		border: {
				top:60,
				right:60,
				bottom:60,
				left:60
		}
		}
	}


const labsFolder = __dirname+'/..';
const fs = require('fs');



fs.readdir(labsFolder, (err, files) => {
	createPdf(files);
});

function createPdf(files){
	if(files[0]){
		let file = files[0]
		if (!file.endsWith(".md")) {
			createPdf(files.slice(1, files.length))
		}
		else{
			console.log(file);
			sourcePath = path.join(labsFolder, file);
			console.log(sourcePath);
			fileOptions = {
				...options,
				source: sourcePath,
				destination: path.join(__dirname + "/../publish", file.substr(0, file.lastIndexOf(".")) + ".pdf"),
			}
			mdpdf.convert(fileOptions).then((pdfPath) => {
				console.log('PDF Path:', pdfPath);
				createPdf(files.slice(1, files.length))
			}).catch((err) => {
				console.error(err);
			});
		}
	};
}