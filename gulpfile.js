const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const cssmin = require('gulp-cssmin');
const postcss = require('gulp-postcss');
const autoprefixer = require('autoprefixer');

// Task để biên dịch SCSS
gulp.task('sass', function () {
    return gulp.src('./Assests/scss/site.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(postcss([autoprefixer()]))
        .pipe(cssmin())
        .pipe(gulp.dest('wwwroot/css'));
});

// Task mặc định
gulp.task('default', gulp.series('sass'));