const resolve = require('path').resolve
const { Command } = require('commander')

const program = new Command()

const type = {
  path(value) { return resolve(value) },
  float(value) { return parseFloat(value) },
  int(value) { return parseInt(value, 10) }
}

program
  .name('TCATimer')
  .version('1.0.0')

  // TODO remove when squirrel is not used anymore!
  .allowUnknownOption()

  .option('--url <name>', 'set timer URL',
    'https://timer.tcanationals.com/tca1')

  .option('--debug', 'set debug flag', !!process.env.TROPY_DEBUG)


module.exports = {
    parse: (argv = process.argv.slice(1)) => {
        program.parse(argv, { from: 'user' })
      
        return {
          opts: program.opts(),
          args: program.args
            // TODO remove with allowUnknownOption!
            .filter(arg => !arg.startsWith('-'))
        }
    }
}
