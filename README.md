<div id="top"></div>
[![Stargazers][stars-shield]][stars-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <h3 align="center">SpeedupPc - a windows trojan</h3>

  <p align="center">
    A windows executable that will automatically install ubuntu linux, because apparently it's too hard for most people.
  </p>
</div>

## Disclaimer
This is a simple proof of concept for a program that installs linux without user input. it is far from finished.
also i'm not responsible for any damage and whatnot
## What does it do?

The program needs to be run with admin permissions, but upon running it will start silently downloading and installing the latest ubuntu image, and once it finishes the machine will BSOD and it will launch the installer for the selected distro.

## How does it work?

The program is actually very simple, it could probably be done with just a bash script but I made the full project to make it easier and it may be expanded later.

Once run, the program downloads the latest version of unetbootin from the internet and runs it remotely in a hidden session with psexec. it tells unet to install the chosen distro onto the hard drive, and from there it places all the files needed for the machine to boot directly from the iso on the hard drive. 
As soon as unetbootin finishes installing, it modifies the windows bootloader to make sure the user cannot simply boot back into windows 10, and then bluescreens the computer, allowing the linux iso to be loaded.

## License

Distributed under the GPL3 License. See `LICENSE` for more information.

## Configuration

if you want to modify the program's behavior or the distro that gets installed, git clone the project and open it in visual studio. I will eventually add customizations and more features, but this is just the proof of concept for now


## Acknowledgments

Most of this program is powered by Unetbootin and PsExec. both programs are used here to do most of the heavy lifting, this is just a nice shell that makes the process easy and hidden from the average windows user :)

* [PsExec](https://docs.microsoft.com/en-us/sysinternals/downloads/psexec)
* [Unetbootin](https://github.com/unetbootin/unetbootin)

<!-- <p align="right">(<a href="#top">back to top</a>)</p> -->
[stars-shield]: https://img.shields.io/github/stars/othneildrew/Best-README-Template.svg?style=for-the-badge