# PrimitierOnlineMod

## About The Project

This project was created to create a mod to add a multiplayer mode to the VR sandbox game [Primitier](https://store.steampowered.com/app/1745170/Primitier/).

## Getting Started - Docker

### Prerequisites

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

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Acknowledgments

We would like to thank everyone involved in this project.

- [Ootamato](https://github.com/forte1st)
- [Xgames123](https://github.com/Xgames123)
- [Seva167](https://github.com/Seva167)
