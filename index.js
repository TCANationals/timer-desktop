const electron = require('electron')
const { app, BrowserWindow } = require('electron')
const { parse } = require('./args')

const TrayGenerator = require('./TrayGenerator')

const { args, opts } = parse()

const displayWidth = 400
const displayHeight = 100

let initUrl = 'https://timer.tcanationals.com/tca1'
let mainWindow = null
let trayIcon = null

// Disable HTTP cache
app.commandLine.appendSwitch ("disable-http-cache")

const getDisplayDimensions = () => {
  const display = electron.screen.getPrimaryDisplay()
  const width = display.workArea.width
  const height = display.workArea.height
  return { width, height }
}

const getWindowPosition = () => {
  const windowBounds = getDisplayDimensions()
  const x = Math.round(windowBounds.width - displayWidth)
  const y = Math.round(windowBounds.height - displayHeight - 2) // Include 2 pixel buffer
  return { x, y }
}

const updateDisplayPosition = () => {
  const position = getWindowPosition()
  mainWindow.setBounds({
    width: displayWidth,
    height: displayHeight,
    x: position.x,
    y: position.y
  });
}

const setupDisplayChangeCheck = () => {
  // Monitor for display changes and update if one happens
  let lastHeight = 0, lastWidth = 0;
  electron.screen.on('display-metrics-changed', (e, d, c) => {
    updateDisplayPosition()
  });
  electron.screen.on('display-added', (e, d) => {
    updateDisplayPosition()
  });
  electron.screen.on('display-removed', (e, d) => {
    updateDisplayPosition()
  });
  setInterval(() => {
    let d = electron.screen.getPrimaryDisplay()
    if (d.workArea.height !== lastHeight || d.workArea.width !== lastWidth) {
      lastHeight = d.workArea.height
      lastWidth = d.workArea.width
      updateDisplayPosition()
    }
  }, 2000);
}

const createMainWindow = () => {
  const position = getWindowPosition()

  mainWindow = new BrowserWindow({
    backgroundColor: '#00ffffff',
    width: displayWidth,
    height: displayHeight,
    resizable: false,
    frame: false,
    transparent: true,
    x: position.x,
    y: position.y,
    skipTaskbar: true,
    webPreferences: {
      nodeIntegration: true,
    }
  })
  
  mainWindow.setAlwaysOnTop(true, 'pop-up-menu')
  mainWindow.setIgnoreMouseEvents(true)
  setupDisplayChangeCheck()

  // Ensure no overflow is shown
  mainWindow.webContents.on('did-finish-load', function() {
    mainWindow.webContents.insertCSS('html, body{ overflow: hidden !important;}')
  });

  mainWindow.loadURL(initUrl)
}

app.on('ready', () => {
  setTimeout(function() {
    runStartup()
  }, 300)
})

app.on('window-all-closed', () => {
  app.quit()
})

console.log(args)
console.log(opts)

if (opts.url) {
  initUrl = opts.url
}

const runStartup = () => {
  createMainWindow()
  trayIcon = new TrayGenerator(mainWindow)
  trayIcon.createTray()
}
