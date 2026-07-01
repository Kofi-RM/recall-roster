const fs = require('fs');
const path = require('path');

// Source directory (current directory)
const sourceDir = path.resolve(__dirname);

// Destination directory
const destDir = path.join(sourceDir, 'css');

// Create the destination directory if it doesn't exist
if (!fs.existsSync(destDir)) {
  fs.mkdirSync(destDir);
}

// Read the files in the source directory
fs.readdir(sourceDir, (err, files) => {
  if (err) {
    console.error('Error reading the directory:', err);
    return;
  }

  // Filter out the .css files and move them
  files.filter(file => file.endsWith('.css')).forEach(file => {
    const oldPath = path.join(sourceDir, file);
    const newPath = path.join(destDir, file);

    fs.rename(oldPath, newPath, err => {
      if (err) {
        console.error(`Error moving file ${file}:`, err);
      } else {
        console.log(`Moved ${file} to css folder.`);
      }
    });
  });
});
