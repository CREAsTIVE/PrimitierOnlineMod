[![Version][version-shield]][version-url]
[![GameVersion][gameVersion-shield]][gameVersion-url]
[![Download][download-shield]][download-url]
[![Discord][discord-shield]][discord-url]

<div align="center">
<!--   <a href="https://github.com/othneildrew/Best-README-Template">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a> -->

  <h3 align="center">PrimitierOnlineMod</h3>

  <p align="center">
    Mod to add a multiplayer mode to Primitier.
    <br />
    <a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/wiki"><strong>View the wiki »</strong></a>
    <br />
    <br />
    <a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    ·
    <a href="https://github.com/YutoMaeda1209/PrimitierOnlineMod/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>

## About The Project

This project was created to create a mod to add a multiplayer mode to the VR sandbox game [Primitier](https://store.steampowered.com/app/1745170/Primitier/).

## Getting Started

How to build a server using Docker.

### Prerequisites

Install the tools needed to build the server.

- git
  ```sh
  apt install git
  ```
- docker
  ```sh
  apt install docker
  ```

### Installation

1. Clone GitHub repository.
   ```sh
   git clone https://github.com/YutoMaeda1209/PrimitierOnlineMod.git
   ```
3. Edit the `Server/settings.json` file.
  > [!NOTE]
  > If you change the Port parameter, change the port number in the command arguments in section 4.
3. Create a Docker image.
   ```sh
   docker build -t pomserver-image -f .\Dockerfile .
   ```
4. Run a Docker container.
   ```sh
   docker run --rm -d -p 54162:54162/udp pomserver-image
   ```

## Getting Started - Other

If you want to output an executable file or similar, import this repository into Visual Studio and build it.

### Prerequisites

- [Visual Studio](https://visualstudio.microsoft.com/)

## Roadmap

- [x] Connection process to server
- [ ] Transmission of data to the server
  - [x] Data lightweighting (MessagePack)
  - [x] Transmission of lightweight data
  - [ ] Transmission of avatar and world data

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

[version-shield]: https://img.shields.io/github/v/release/YutoMaeda1209/PrimitierOnlineMod?style=for-the-badge
[version-url]: https://github.com/YutoMaeda1209/PrimitierOnlineMod/releases
[gameVersion-shield]: https://img.shields.io/badge/Primitier-v1.9.0-limegreen?style=for-the-badge
[gameVersion-url]: https://store.steampowered.com/app/1745170/Primitier/
[download-shield]: https://img.shields.io/badge/Download-latest-blue?style=for-the-badge
[download-url]: https://github.com/YutoMaeda1209/PrimitierOnlineMod/releases/latest
[discord-shield]: https://img.shields.io/badge/-Discord-gray?style=for-the-badge&logo=Discord&logoColor=white
[discord-url]: https://discord.com/channels/968161559387979876/1262816599174549524
