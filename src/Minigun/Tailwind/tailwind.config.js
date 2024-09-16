/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        '../**/*.cshtml'
    ],
    theme: {
        extend: {
            colors: {
                'primary-navy-dark' : '#062A47',
                'primary-raygun-blue' : '#2C9AF0',
                'secondary-blue-grey' : '#CFD8DC',
            }
        },
    },
    plugins: [
        require('@tailwindcss/typography')
    ],
}