[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

# PrimitierOnlineMod

## About The Project

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
2. Edit the `Server/settings.json` file.

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
