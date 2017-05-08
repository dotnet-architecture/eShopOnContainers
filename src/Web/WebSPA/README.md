# eShop Web SPA (Single Page Application)

## Requirements and set up

### Install NPM
The SPA application is using a newer version of NPM than the one provided by Visual Studio 2015 (npm 2.7.4 currently), so you need to install the latest stable version of NPM.

NPM is bundled with NODE.JS. Installing NPM and NODE is pretty straightforward by using the installer package available at https://nodejs.org/en/

<img src="../../../../img/spa/installing_npm_node.png">
You can install the version "Recommended For Most Users" of Node which at the moment of this writing was v6.9.3 LTS and comes with a newer version of NPM.
You can see your initial NPM version and the installed NPM version with the command 
<b>npm -v</b>, as shown below.
<p>
<img src="../../../../img/spa/npm-versions-powershell.png">

### Set NPM path into Visual Studio
NPM will be usually installed under this path:
<b>C:\Program Files (x86)\nodejs</b>.
You need to update that path in Visual Studio 2015 under the "External Web Tools" location paths, as shown below:
<p>
<img src="../../../../img/spa/vs-tools-path-custom-node.png">

### Build the SPA app with NPM
Finally, you need to build the SPA app (TypeScript and Angular based client app) with NPM.
* Open a command-prompt window and move to the root of the SPA application (src\Web\WebSPA\eShopOnContainers.WebSPA)
* Run the command <b>npm run build:prod</b> as shown below:
<p>
<img src="../../../../img/spa/npm-run-build.png">

If you get an error like <b>"Node Sass could not find a binding for your current environment: Windows 64-bit with Node.js 6.x"</b>, then run the command <b>npm rebuild node-sass</b> as in the following screenshot:
<img src="../../../../img/spa/npm-rebuild-node-sass.png">
Then, run again the <b>npm run build:prod</b> command that should finish with no errors.

### Build/create the Docker images
Create the Docker images with the <b>build-images.ps1</b> PowerShell script in Windows (or the <b>build-images.sh</b> bash script in a Mac) as explained in the main instructions at https://github.com/dotnet/eShopOnContainers/ 

### Deploy/run the Docker containers
Deploy/run the Docker containers with <b>"docker-compose up"</b> as explained in the main instructions at https://github.com/dotnet/eShopOnContainers/ 

### Test the SPA web application

Test the SPA app by running the following URL in a browser:
<b> http://TBD</b>


