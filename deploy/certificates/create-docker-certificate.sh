echo "creating base certificate (.pem) and private key (.key) files..."
openssl req \
  -x509 \
  -days 365 \
  -out docker-self-signed.pem \
  -keyout docker-self-signed.key \
  -newkey rsa:2048 -nodes -sha256 \
  -subj '/CN=localhost' \
  -extensions EXT \
  -config <( \
    printf "[dn]\nCN=localhost\n[req]\ndistinguished_name = dn\n[EXT]\nsubjectAltName='DNS.1:localhost,DNS.2:host.docker.internal'\nkeyUsage=digitalSignature\nextendedKeyUsage=serverAuth")

echo "printing text version..."
openssl x509 -in docker-self-signed.pem -text -noout > docker-self-signed.txt

echo "generating certificate container file (.pfx)..."
openssl pkcs12 -export \
  -inkey docker-self-signed.key \
  -in docker-self-signed.pem \
  -out docker-self-signed.pfx \
  -name "Docker development certificate" \
  -password pass:$1
