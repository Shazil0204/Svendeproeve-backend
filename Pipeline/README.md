# GitLab CI pipeline

The root `.gitlab-ci.yml` includes the jobs in this folder.

The runner uses the Shell executor, so the server does not need the .NET SDK.
It does need Docker installed and the `gitlab-runner` user must be allowed to
run Docker commands.

## Backend validation

The `validate` job uses the `backend` runner tag and mounts the checkout into
`mcr.microsoft.com/dotnet/sdk:10.0`. It restores, builds, and tests the backend
inside a temporary container using `docker run --rm`. The `after_script` also
removes the named container, including after a failed command.
