# eShopOnContainers - BRANCH GUIDE

Following are the most important branches:

- `dev`: Contains the latest code **and it is the branch actively developed**. Note that **all PRs must be against `dev` branch to be considered**.
- `master`: Synced time to time from dev. It contains "stable" code, although not the latest one. We plan to do periodic merges from `dev` to `master`, but we are not doing it right now.
- `netcore2`: Contains NETCore 2.0 (preview2) based code. All APIs and webs are migrated to netcore2 except `Identity.API` which still uses netcore1.1 (because of Identity Server). Dockerfiles are updated also. Use this branch to test the NETCore 2.0 code. You can also submit PR to this branch if they are related to netcore2.

Any other branch is considered temporary and could be deleted at any time. Do not do any PR to them!

Thanks!
