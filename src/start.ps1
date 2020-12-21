
if ($args.Count -eq 0) {
    docker-compose.exe -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.certificates.yml up -d
} elseif ($args.Count -eq 1 -and $args[0] -eq "infra") {
    docker-compose.exe -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.certificates.yml up -d seq sqldata nosqldata basketdata rabbitmq
} else {
    docker-compose.exe -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.certificates.yml up -d $args
}
