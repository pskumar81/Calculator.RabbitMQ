# Create directory for certificates if it doesn't exist
New-Item -ItemType Directory -Force -Path "ca"
New-Item -ItemType Directory -Force -Path "server"
New-Item -ItemType Directory -Force -Path "client"

# Generate CA key and certificate
openssl genpkey -algorithm RSA -out ca/ca.key
openssl req -new -x509 -key ca/ca.key -out ca/ca.crt -subj "/CN=Calculator CA"

# Generate Server key and CSR
openssl genpkey -algorithm RSA -out server/server.key
openssl req -new -key server/server.key -out server/server.csr -subj "/CN=calculator-server"

# Generate Client key and CSR
openssl genpkey -algorithm RSA -out client/client.key
openssl req -new -key client/client.key -out client/client.csr -subj "/CN=calculator-client"

# Sign Server Certificate with CA
openssl x509 -req -in server/server.csr -CA ca/ca.crt -CAkey ca/ca.key -CAcreateserial -out server/server.crt -days 365

# Sign Client Certificate with CA
openssl x509 -req -in client/client.csr -CA ca/ca.crt -CAkey ca/ca.key -CAcreateserial -out client/client.crt -days 365
