const electron = require('electron')
const { app, BrowserWindow } = require('electron')

function createWindow () {

  const win = new BrowserWindow({
    width: 1000,
    height: 800,
    webPreferences: {
      nodeIntegration: true
    }
  })

  win.loadURL('https://timer.tcanationals.com/tca1')
}

app.whenReady().then(createWindow)

app.on('window-all-closed', () => {
  app.quit()
})
