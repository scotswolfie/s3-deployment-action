# AWS S3 Deployment Action 🚀

[![Build](https://github.com/scotswolfie/s3-deployment-action/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/scotswolfie/s3-deployment-action/actions/workflows/build.yml)
[![Build and update the binary](https://github.com/scotswolfie/s3-deployment-action/actions/workflows/update-dist.yml/badge.svg?branch=main)](https://github.com/scotswolfie/s3-deployment-action/actions/workflows/update-dist.yml)

## Overview

This GitHub Action makes it easy to upload all files from a directory to an AWS
S3 bucket and create a GitHub Deployment at the same time.

It was created with the intention of solving a particular issue, so if there's
any functionality that's currently missing, and you'd like to see added in the
future, feel free to reach out! Any other feedback or suggestions are
welcome as well ✌️

## Inputs

| Input name               | Required                                  | Type    | Description                                                                                                                               |
|--------------------------|-------------------------------------------|---------|-------------------------------------------------------------------------------------------------------------------------------------------|
| `aws-access-key`         | Yes                                       | string  | Access key for the AWS API.                                                                                                               |
| `aws-secret-access-key`  | Yes                                       | string  | Secret access key for the AWS API.                                                                                                        |
| `aws-region`             | Yes                                       | string  | AWS region used by the S3 client (e.g. `us-east-1`).                                                                                      |
| `bucket-name`            | Yes                                       | string  | Name of the destination bucket.                                                                                                           |
| `deployment-log-url`     | No                                        | string  | Overrides the deployment log address assigned to GitHub Deployment (_default:_ URL of the action's workflow).                             |
| `environment-name`       | No                                        | string  | Name of the deployment environment (_default:_ `production`).                                                                             |
| `environment-url`        | No                                        | string  | Deployment URL to be assigned to the GitHub Deployment. Use `{prefix}` to replace it with the final object prefix of the uploaded files.  |
| `github-token`           | Only when `skip-github-deployment: false` | string  | API token to access GitHub. Needs to have at least write permissions to deployments.                                                      |
| `git-ref`                | No                                        | string  | Overrides the git reference assigned to deployment (_default:_ value of the `GITHUB_REF` environment variable).                           |
| `log-level`              | No                                        | string  | How verbose should the action be (_default:_ `info`). See [Verbosity Levels](#verbosity-levels) for a list of allowed values.             |
| `object-prefix`          | No                                        | string  | Prefix used for the keys of uploaded objects. Can be used to create a directory structure.                                                |
| `object-prefix-guid`     | No                                        | boolean | Should the objects uploaded in a single run be prefixed by GUID (_default:_ `true`).                                                      |
| `production-environment` | No                                        | boolean | Indicates if the deployment environment is a production one (_default:_ `true` if `environment-name: production`, `false` otherwise).     |
| `s3-endpoint`            | No                                        | string  | Can be specified to override the endpoint URL for S3 client (only for advanced use cases).                                                |
| `skip-github-deployment` | No                                        | boolean | Should the action omit creating a GitHub Deployment and only upload files to S3 (_default:_ `false`).                                     |
| `source-directory`       | Yes                                       | string  | Path to the directory containing files to upload. Can be relative or absolute. The name of the directory won't be included in the prefix. |

## Outputs

| Output name          | Description                                                                                                                 |
|----------------------|-----------------------------------------------------------------------------------------------------------------------------|
| `deployment-id`      | ID of the GitHub Deployment that was created (if any).                                                                      |
| `environment-url`    | The final environment URL after a potential replacement of the prefix placeholder (if one was provided in the first place). |
| `object-prefix-guid` | GUID used to prefix the S3 object keys (if any).                                                                            |
| `object-prefix`      | Complete prefix used in the S3 object keys (if any).                                                                        |

## Verbosity Levels

| Level     | Description                                                                                                                               |
|-----------|-------------------------------------------------------------------------------------------------------------------------------------------|
| `off`     | Disables all logging to the console.                                                                                                      |
| `error`   | Error messages only.                                                                                                                      |
| `warning` | Error and warning messages.                                                                                                               |
| `info`    | Default level, includes error, warning, and information messages.                                                                         |
| `verbose` | The most detailed level, displays all other messages, as well as some additional output (e.g. details about requests sent to GitHub API). |

## Example usage

```yaml
permissions:
  contents: read
  deployments: write
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
      - name: S3 Deployment
        uses: scotswolfie/s3-deployment-action@v1
        with:
          aws-access-key: ${{ secrets.AWS_ACCESS_KEY }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: "eu-west-1"
          bucket-name: "example-bucket"
          environment-url: "https://example.com/{prefix}index.html"
          github-token: ${{ secrets.GITHUB_TOKEN }}
          source-directory: "./dist"
          object-prefix: "public"
```