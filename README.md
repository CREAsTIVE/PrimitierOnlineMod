
<p align=center>
    <a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/releases/latest"><img src="https://img.shields.io/badge/Download-latest-blue?style=for-the-badge"/></a>
    <a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/releases"><img src="https://img.shields.io/github/v/release/YutoMaeda1209/PrimitierOnlineMod?style=for-the-badge"/></a>
    <a href="https://store.steampowered.com/app/1745170/Primitier/"><img src="https://img.shields.io/badge/Primitier-v1.9.0-limegreen?style=for-the-badge"/></a>
    <a href="https://discord.com/channels/968161559387979876/1262816599174549524"><img src="https://img.shields.io/badge/-Discord-gray?style=for-the-badge&logo=Discord&logoColor=white"/></a>
</p>

<div align="center">
<!--   <a href="https://github.com/othneildrew/Best-README-Template">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a> -->

  <h3 align="center">PrimitierOnlineMod</h3>

  <p align="center">
    Mod to add a multiplayer mode to Primitier.
<!--     <br />
    <a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/wiki"><strong>View the wiki »</strong></a>
    <br />
    <br />
    <a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    ·
    <a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a> -->
  </p>
</div>

<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started-with-client">Getting Started with Client</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started-with-server">Getting Started with Server</a>
      <ul>
        <li><a href="#prerequisites-1">Prerequisites</a></li>
        <li><a href="#installation-1">Installation</a></li>
      </ul>
    </li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

## About The Project

This project was created to create a mod to add a multiplayer mode to the VR sandbox game [Primitier](https://store.steampowered.com/app/1745170/Primitier/).

## Getting Started with Client

How to install the mod to Primitier.

### Prerequisites

Install the tools needed to install the mod.

- [MelonLoader](https://melonwiki.xyz/)

### Installation

1. download the mod from the release page.
2. put `PrimitierOnlineMod.dll` and `settings.json` in Primitier/Mods folder.
3. Play Primitier!

## Getting Started with Server

How to build a server using Docker.

### Prerequisites

Install the tools needed to build the server.

- Git
  ```sh
  apt install git
  ```
- Docker
  ```sh
  apt install docker
  ```

### Installation

1. Clone GitHub repository.
   ```sh
   git clone https://github.com/YutoMaeda1209/PrimitierOnlineMod.git
   ```
2. Edit the `Server/settings.json` file.
  > [!NOTE]
  > If you change the Port parameter, change the port number in the command arguments in section 4.
3. Create a Docker image.
   ```sh
   docker build -t pom-image -f .\Dockerfile .
   ```
4. Run a Docker container.
   ```sh
   docker run --name pomServer --mount 'src=pomVolume,dst=/mnt/pom' -d -p 54162:54162 -p 54162:54162/udp pom-image
   ```
  > [!TIP]
  > If you want to destroy the container after it has been terminated, replace `--name pomServer` with `--rm`.

## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feat/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feat/AmazingFeature`)
5. Open a Pull Request

### Top contributors:

<a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=YutoMaeda1209/PrimitierOnlineMod" alt="contrib.rocks image" />
</a>

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

YutoMaeda - [Discord Discussion](https://discord.com/channels/968161559387979876/1262816599174549524)

## Acknowledgments

We would like to thank everyone involved in this project.

- [Ootamato](https://github.com/forte1st)
- [Xgames123](https://github.com/Xgames123)
- [Seva167](https://github.com/Seva167)
