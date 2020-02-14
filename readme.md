# Map Dot Generator

This project consists of a simple background worker project. The background worker generates random map points using [Bogus](), the awesome fake-data generator. You can open this project up in Visual Studio Online by clicking this button. 

[![Open in Visual Studio Online](https://img.shields.io/endpoint?style=social&url=https%3A%2F%2Faka.ms%2Fvso-badge)](https://online.visualstudio.com/environments/new?name=Ballpark%20Tracker&repo=bradygaster/MapDotGenerator)

As it generates these points it sends them over to a SignalR Hub running in another microservices (represented by [the MapDotUi]() repository source code. 
