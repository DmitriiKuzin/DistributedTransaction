# Otus-ApiGateWay

```shell
helm install otus .\deploy\distributed-transaction\ --namespace otus-dt --create-namespace
```

```shell
newman run .\idempotency.postman_collection.json
```