# SentryVision
An IoT based, open-source, camera-feed system.

## Features
- TBD

## Installation
- TBD

## Systems Design
The system is designed to be modular, with each component being able to be swapped out for another. It comprises an endpoint software, an API, and a web interface.

The endpoint software is designed to take in the video feed and stream it to the main server, while also dealing with end-user authentication.
The server takes in camera feeds from the endpoints and stores them in a database, while also providing an API for the web interface to use. The server uses one-time token based authentication to ensure that the user is connecting to a known endpoint.
The web interface is designed to be a simple, easy to use interface for the user to view the camera feeds and modify settings. It is a shell for the functions exposed by the API.

All these are modular and can be swapped out with another solution, as long it does not include any breaking changes (unless they are dealt with in the other components as well).

![systems_design.png](images%2Fsystems_design.png)