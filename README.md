# ShutDown
Timed shut down of your Windows PC

### V 1.3 changes

- If app is closed to tray, starting the app will bring existing instance of the app on top instead of starting a new one
- Time gap detection, If app is  running a delayed operation (e.g. shut-down) and user manually puts the pc to sleep, next time when windows is started app will detect time gap and cancel the operation instead of executing it
- Closing app to tray was not working unless user clicked on X button on main window, it is now fixed
- New version detection (must be enabled in settings)
- Minor spelling fixes

### V 1.2 changes

- Mouse Jiggling added

---

### V 1.1 changes

- Memory usage optimization
- Prevent screen turning off
- Some minor fixes

---

![ShutDown](http://stanac.github.io/shutdown/images/ss-101-1.png)

![ShutDown](http://stanac.github.io/shutdown/images/ss-101-2.png)

![ShutDown](http://stanac.github.io/shutdown/images/ss-101-3.png)

If you are using Visual Studio 2015 and failing to load the project ShutDown.Setup, you need install [Wix Toolset Extension](https://marketplace.visualstudio.com/items?itemName=RobMensching.WixToolsetVisualStudio2015Extension)

For download visit [GitHub Pages](http://stanac.github.io/shutdown/) 
