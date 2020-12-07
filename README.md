# Falling Sand Demo

I saw this [GDC talk](https://youtu.be/prXuyMCgbTc) by one of the Noita devs. Early on he explains the algorithm used to simulate falling particles and it was so simple that I decided to give it a try in [Monogame](https://www.monogame.net/).

Since the sand grains are the size of pixels I apply scaling using a separate render target, see [this StackOverflow post](https://stackoverflow.com/a/7603116/13900323) for more details.

I want this project to be a playground for learning new things in Monogame. You can open this project in VS Code and launch it as a .NET Core project or build it on the command line using `dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`, from the [Monogame docs](https://docs.monogame.net/articles/packaging_games.html).

### Things to try:
* Try adjusting `size_x` and `size_y` to change the scaling. Smaller numbers will result in larger pixels but a smaller number of available sand grains.
* Try changing the speed the game runs at by changing `TargetElapsedTime`. A smaller time per frame should result in faster runtime.
* Try changing the color of the sand from `Colors.SandyBrown` to something else.

This code is CC0, public domain.