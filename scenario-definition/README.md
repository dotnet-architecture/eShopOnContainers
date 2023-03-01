# Scenario Definition for EshopOnContainers

To use the simulation framework for microservices you have
to specify a scenario/workload using json (configuration) files.

The configuration files consist of:

- operations: a HTTP request 
- transactions: multiple operations sequentially chained together
- scenario: specifies which transactions are run and other workload
  configuration like distribution, arguments the transaction should use etc.
  
  
The scenario should simulate a workload the microservice application would
experience during a given timespan in production.



## Scenario for eshopOnContainers

Arguments and Dynamic Variables for Transactions:
- userId
- productId


Transactions:
- Customer reads items, adds item to basket, checkouts basket
- Customer adds item to basket, removes again, logs out
- Price update transaction
- Stock replenished for catalog item
- Catalog item is removed
- create Order draft
- Order cancel



Workload configuration:
- dataskew on catalog items (distribution of which items are accessed/bought)
- distribution between different transactions
- how many concurrent transactions
- how many total transactions to be executed

## Realistic Scenario Description for eShop

Run multiple benchmarks / experiments with varying factor.

Customer buys items
