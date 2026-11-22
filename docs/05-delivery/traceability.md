# Traceability Matrix

Every requirement sub-ID → the story that delivers it → the test(s) that prove it. **Update the Tests column in the same PR as the story.** In P9-01 this file is regenerated from `[Trait("Req", …)]` attributes and the build fails if a row has no test.

Test naming: `<ReqId with underscores>_<behaviour>`, e.g. `INV_2_2_reserving_more_than_available_is_rejected`.

## Inventory

| Req | Story | Tests |
| --- | --- | --- |
| INV-1.1 | P2-01 | |
| INV-1.2 | P2-01 | |
| INV-1.3 | P2-01, P3-05 | |
| INV-1.4 | P3-05, P7-03 | |
| INV-2.1 | P2-02 | |
| INV-2.2 | P2-02 | |
| INV-2.3 | P2-02 | |
| INV-2.4 | P2-02 | |
| INV-2.5 | P2-02, P3-05 | |
| INV-2.6 | P3-05 | |
| INV-3.1 | P2-03 | |
| INV-3.2 | P2-03, P4-05 | |
| INV-3.3 | P2-03 | |
| INV-3.4 | P2-03, P5-05 | |
| INV-3.5 | P5-05 | |
| INV-4.1 | P2-04 | |
| INV-4.2 | P2-04 | |
| INV-4.3 | P2-04 | |
| INV-4.4 | P5-05 | |
| INV-5.1 | P2-05 | |
| INV-5.2 | P2-05 | |
| INV-5.3 | P2-05 | |
| INV-5.4 | P2-05 | |
| INV-6.1 | P2-06 | |
| INV-6.2 | P2-06 | |
| INV-6.3 | P2-06 | |
| INV-6.4 | P2-06 | |
| INV-6.5 | P2-06 | |
| INV-7.1 | P2-07 | |
| INV-7.2 | P2-07 | |
| INV-7.3 | P2-07 | |
| INV-8.1 | P2-08 | |
| INV-8.2 | P2-08 | |
| INV-8.3 | P2-08, P7-03 | |
| INV-8.4 | P2-08 | |
| INV-8.5 | P2-08 | |

## Ordering

| Req | Story | Tests |
| --- | --- | --- |
| ORD-1.1 | P3-01 | |
| ORD-1.2 | P3-01 | |
| ORD-1.3 | P3-01 | |
| ORD-1.4 | P3-01 | |
| ORD-1.5 | P3-04, P6-02 | |
| ORD-1.6 | P3-01 | |
| ORD-1.7 | P3-01 | |
| ORD-2.1 | P3-02 | |
| ORD-2.2 | P3-02 | |
| ORD-2.3 | P3-02 | |
| ORD-2.4 | P3-02 | |
| ORD-3.1 | P3-03 | |
| ORD-3.2 | P3-03 | |
| ORD-3.3 | P3-03 | |
| ORD-3.4 | P3-03 | |
| ORD-4.1 | P3-03 | |
| ORD-4.2 | P3-03 | |
| ORD-4.3 | P3-03 | |
| ORD-5.1 | P3-02, P6-02 | |
| ORD-5.2 | P3-02 | |
| ORD-5.3 | P3-01 | |
| ORD-5.4 | P3-02, P6-02 | |

## Fulfillment

| Req | Story | Tests |
| --- | --- | --- |
| FUL-1.1 | P5-01, P5-02 | |
| FUL-1.2 | P5-02 | |
| FUL-2.1 | P5-01 | |
| FUL-2.2 | P5-01 | |
| FUL-2.3 | P5-01 | |
| FUL-3.1 | P5-01 | |
| FUL-3.2 | P5-01 | |
| FUL-3.3 | P5-01 | |
| FUL-4.1 | P5-01, P5-02 | |
| FUL-4.2 | P5-01, P5-02 | |

## Catalog and Billing

| Req | Story | Tests |
| --- | --- | --- |
| CAT-1.1 | P6-01 | |
| CAT-1.2 | P6-01 | |
| CAT-1.3 | P6-01 | |
| CAT-2.1 | P6-01 | |
| CAT-2.2 | P6-02 | |
| BIL-1.1 | P6-03 | |
| BIL-1.2 | P6-03 | |
| BIL-1.3 | P6-03 | |
| BIL-2.1 | P1-03, P6-03 | |
| BIL-3.1 | P6-03 | |
| BIL-3.2 | P6-03 | |

## Saga

| Req | Story | Tests |
| --- | --- | --- |
| SAGA-1.1 | P5-03, P5-06 | |
| SAGA-1.2 | P5-03, P5-06 | |
| SAGA-1.3 | P5-03, P5-06 | |
| SAGA-2.1 | P5-03, P5-06 | |
| SAGA-3.1 | P5-03, P5-06 | |
| SAGA-4.1 | P5-03, P5-06 | |
| SAGA-4.2 | P5-03, P5-06 | |
| SAGA-5.1 | P5-03, P5-06 | |
| SAGA-6.1 | P5-03, P5-06 | |
| SAGA-6.2 | P5-03, P5-06 | |
| SAGA-6.3 | P5-03, P5-06 | |
| SAGA-7.1 | P5-03, P5-06, P8-01, P8-03 | |

## API and dashboard

| Req | Story | Tests |
| --- | --- | --- |
| API-1 | P7-02 | |
| API-2 | P7-02 | |
| API-3 | P7-01 … P7-04 | |
| API-4 | P7-03 | |
| API-5 | P7-03 | |
| API-6 | P7-05 | |
| API-7 | P7-01, P8-01 | |
| DASH-1 | P8-01, P8-02 | |
| DASH-2 | P8-02, P8-03 | manual check + architecture test (Dashboard references no module) |
| DASH-3 | P8-01 | |

## Non-functional

| Req | Story | Tests |
| --- | --- | --- |
| NFR-1 | P1-01, P1-03 | `ProjectStructureTests`: NFR_1_solution_contains_every_project_in_the_architecture_layout, NFR_1_domain_projects_reference_only_the_shared_kernel, NFR_1_shared_kernel_and_building_blocks_contracts_reference_nothing, NFR_1_contracts_projects_reference_only_building_blocks_contracts, NFR_1_building_blocks_application_references_only_shared_kernel_and_building_blocks_contracts, NFR_1_application_projects_reference_only_own_domain_contracts_and_building_blocks, NFR_1_infrastructure_projects_reference_only_own_application_and_building_blocks_infrastructure, NFR_1_reporting_references_only_contracts_and_building_blocks, NFR_1_catalog_references_only_own_contracts_and_building_blocks, NFR_1_dashboard_references_no_solution_project, NFR_1_api_host_composes_every_module_and_the_dashboard |
| NFR-2 | P1-03 | |
| NFR-3 | P3-05, P5-04 | |
| NFR-4 | P4-02, P4-04 | |
| NFR-5 | P4-03, P4-04 | |
| NFR-6 | P1-03, P9-01 | |
| NFR-7 | P1-02 | |
| NFR-8 | P1-03 | |
| NFR-9 | P9-01 | review |
| NFR-10 | P4-01, P4-04 | |
| NFR-11 | P4-03, P7-01 | |

## Architecture decisions (scaffold checks)

Not requirements, but stories cite them, so their tests carry `[Trait("Req", "ADR-xxxx")]`.

| ADR | Story | Tests |
| --- | --- | --- |
| ADR-0013 | P1-01 | `ProjectStructureTests`: ADR_0013_package_versions_are_managed_centrally, ADR_0013_all_projects_build_net9_with_strict_settings_from_directory_build_props; `HealthEndpointTests`: ADR_0013_health_endpoint_returns_ok; `TestProjectDiscoveryTests`: ADR_0013_test_project_is_discovered_by_the_runner (one per test project) |
| ADR-0020 | P1-01 | `ProjectStructureTests`: ADR_0020_sqlite_provider_is_referenced_by_building_blocks_infrastructure_and_never_by_inner_layers |
