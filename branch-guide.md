# eShopOnContainers - BRANCH GUIDE

Following are the most important branches:

- `dev`: Contains the latest code **and it is the branch actively developed**. Note that **all PRs must be against the `dev` branch to be considered**. This branch is developed using `.NET 7`
- `release/net-6`: Contains the code changes specific to the `.NET 6`
- `release/net-5`: Contains the code changes specific to the `.NET 5`
- `release/net-3.1.1`: Contains the code changes specific to the `.NET 3.1`

> [!DISCLAIMER]: The `main` branch contains the old code base and will get obsolete in the future. So it's recommended to refer to different [tags](https://github.com/dotnet-architecture/eShopOnContainers/tags) to avoid any confusion. 

Any other branch is considered temporary and could be deleted at any time. Do not submit any PR against them!

Thanks!
