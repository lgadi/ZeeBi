import System.Environment 
import System.IO

#import GitSharp 

projectName = "ZeeBi.UI"

#repo as Repository
version as string
srcRoot = "..\\src\\"
slnFile = Path.Combine(srcRoot, "${projectName}.sln");
configuration = 'Release'
rootDir = Directory.GetCurrentDirectory()
outDir = Path.Combine(rootDir, "out")

#def getRepo:
#	if repo == null:
#		repo = Repository(Directory.GetCurrentDirectory())
#	return repo 

def writeBuildInfo:
	sourceOutFilename = Path.Combine(outDir, 'BuildInfo.txt')
	lines = ["Build info:","===================="]
	lines.Add(" at: " + System.DateTime.UtcNow)
	lines.Add(" branch: master")
	lines.Add(" head: " + System.Environment.GetEnvironmentVariable('GIT_COMMIT_HEAD'))
	lines.Add(" commits: " + System.Environment.GetEnvironmentVariable('GIT_COMMITS'))
	
	File.WriteAllLines(sourceOutFilename, array(string, lines))

def copyDir(source as string, dest as string, search):
	for dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories):
		Directory.CreateDirectory(dirPath.Replace(source, dest))
		
	for newPath in Directory.GetFiles(source, search, SearchOption.AllDirectories):
		cp(newPath, newPath.Replace(source, dest))

#getRepo()


desc "set build configuration to Debug"
target debug:
	configuration = 'Debug'


desc "initialization"
target init:
	rmdir(outDir)
	mkdir(outDir)
	mkdir(Path.Combine(outDir, "bin"))
	print 'initialized'
	
target default, (init, build, package, deploy):
	print 'default target completed'
	

desc "Build the sources"
target build:
	exec("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe", "/property:Configuration=${configuration} ${slnFile}")

desc "copy artifacts to output directory"
target package:
	writeBuildInfo()

	outBin = Path.Combine(outDir, "bin");
	projectRoot = Path.Combine(srcRoot,  "${projectName}");
	for file in Directory.GetFiles(Path.Combine(projectRoot,  "bin\\")):
		cp(file, Path.Combine(outBin, Path.GetFileName(file)))
	cp(Path.Combine(projectRoot, "global.asax"), Path.Combine(outDir, "global.asax"))
	cp(Path.Combine(projectRoot, "web.config"), Path.Combine(outDir, "web.config"))


	viewsDir = Path.Combine(projectRoot, 'Views');
	dest = Path.Combine(outDir, 'Views');
	copyDir(viewsDir, dest, "*.cshtml")
	cp(Path.Combine(viewsDir, "web.config"), Path.Combine(dest, "web.config"))

	contentDir = Path.Combine(projectRoot, 'Content');
	dest = Path.Combine(outDir, 'Content');
	copyDir(contentDir, dest, "*.*")

	scriptsDir = Path.Combine(projectRoot, 'Scripts');
	dest = Path.Combine(outDir, 'Scripts');
	copyDir(scriptsDir, dest, "*.*")

	
	
desc "deploy to the dojo server"
target deploy:
	source = outDir;
	dest = "C:\\ZeeBi\\website"
	print "copying from ${source} to ${dest}"
	
	for dirPath in Directory.GetDirectories(dest, "*"):
		print "removing dir ${dirPath}"
		Directory.Delete(dirPath, true)
	for filePath in Directory.GetFiles(dest, "*.*"):
		print "deleting file ${filePath}"
		File.Delete(filePath)

	for dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories):
		targetDir = dirPath.Replace(source, dest)
		print "creating dir ${targetDir}"
		Directory.CreateDirectory(targetDir)
		
	for newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories):
		file = Path.GetFileName(newPath)
		targetFile = newPath.Replace(source, dest)
		cp(newPath, targetFile)

	