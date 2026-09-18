# GitLab CI pipeline

The root `.gitlab-ci.yml` includes the jobs in this folder.

## Staging migrations

The `migrate_staging` job appears as a manual action only for pushes to the
`staging` branch. Run it to apply EF Core migrations. Leave it unstarted to
skip migrations; the job is optional and does not block the merge.

Configure these GitLab CI/CD variables for the migration job:

- `ConnectionStrings__DefaultConnection`: the staging PostgreSQL connection string
- `STAGING_APP_BASE_URL`: the staging application base URL

Keep the connection string masked and protected. The `backend` runner tag is
used by both jobs.
