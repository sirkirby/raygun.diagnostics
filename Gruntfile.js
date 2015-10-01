module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    clean: ["dist"],
    nugetpack: {
      debug: {
        src: 'src/Raygun.Diagnostics/*.csproj',
        dest: 'dist/',
        options: {
         properties: "Configuration=Debug;",
         verbose: true
        }
      },
      release: {
      	src: 'src/Raygun.Diagnostics/*.csproj',
        dest: 'dist/',
        options: {
         properties: "Configuration=Release;"
        }
      }
    },
    nugetpush: {
      dist: {
        src: 'dist/*.nupkg'
      }
    }
  });

  grunt.loadNpmTasks('grunt-contrib-clean');

  // Load the plugin that provides the "nuget" task.
  grunt.loadNpmTasks('grunt-nuget');

  // Default task(s).
  grunt.registerTask('default', ['clean', 'nugetpack:debug']);
  grunt.registerTask('release', ['clean', 'nugetpack:release']);
  // Build and publish to nuget task
  grunt.registerTask('publish', ['clean', 'nugetpack', 'nugetpush']);
};
