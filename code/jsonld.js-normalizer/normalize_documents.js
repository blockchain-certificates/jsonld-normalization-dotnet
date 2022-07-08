
const jsonld = require('jsonld');
const fs = require('fs');
const path = require('path');

async function normalizeDocument (document) {    
    const normalizeArgs = {
      algorithm: 'URDNA2015',
      format: 'application/nquads',
      documentLoader: jsonld.documentLoader
    };
  
    return await jsonld.normalize(document, normalizeArgs);
}

function normalizeFile(fileName) {
  fs.readFile(fileName, (err, buffer) => {
    if (err) {
      console.error(`could not read file (${fileName}): ${err}`);
    } else {
      const documentStr = typeof buffer === 'string' ? buffer : buffer.toString();
      const document = JSON.parse(documentStr);
      console.log(`normalizing document ${fileName}`);
      normalizeDocument(document).then(normalized => {
        if (normalized || normalized === '') {
          const directory = `${path.dirname(fileName)}/normalized`;
          if (!fs.existsSync(directory)) {
            fs.mkdirSync(directory);
          }
          const resFileName = path.basename(fileName).replace('-in.jsonld', '-out.nq');
          const resPath = `${directory}/${resFileName}`;
          try {
            fs.writeFileSync(resPath, normalized);
          } catch (e) {
            console.error(`failed to save the result file (${resPath}): ${e}`);
          }
        } else {
          console.error(`process failed unexpectedly`)
        }
      }).catch(e => {
        console.error(`file normalization failed: ${e}`);
      });
    }
  });
}

const args = process.argv;
if (args.length < 3) {
  console.error('name of the jsonld file (or a whole folder) to normalize is required as a parameter');
} else {
  const paramValue = args[2];
  const fileStat = fs.lstatSync(paramValue);
  if (fileStat.isDirectory()) {
    const files = fs.readdirSync(paramValue);
    for (const fileName of files) {
      normalizeFile(`${paramValue}/${fileName}`);
    }
  } else if (fileStat.isFile()) {
    normalizeFile(paramValue);
  } else {
    console.error('passed parameter needs to be a file or a directory')
  }
}
