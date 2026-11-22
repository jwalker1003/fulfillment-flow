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
| BIL-2.1 | P1-03, P6-03 | `BillingProviderIsolationTests`: BIL_2_1_billing_domain_and_application_types_do_not_depend_on_a_fakepay_namespace, BIL_2_1_billing_domain_and_application_reference_no_fakepay_assembly (rule only; adapter in P6-03) |
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
| NFR-1 | P1-01, P1-03 | `ProjectStructureTests`: NFR_1_solution_contains_every_project_in_the_architecture_layout, NFR_1_domain_projects_reference_only_the_shared_kernel, NFR_1_shared_kernel_and_building_blocks_contracts_reference_nothing, NFR_1_contracts_projects_reference_only_building_blocks_contracts, NFR_1_building_blocks_application_references_only_shared_kernel_and_building_blocks_contracts, NFR_1_application_projects_reference_only_own_domain_contracts_and_building_blocks, NFR_1_infrastructure_projects_reference_only_own_application_and_building_blocks_infrastructure, NFR_1_reporting_references_only_contracts_and_building_blocks, NFR_1_catalog_references_only_own_contracts_and_building_blocks, NFR_1_dashboard_references_no_solution_project, NFR_1_api_host_composes_every_module_and_the_dashboard; `DomainPurityTests`: NFR_1_domain_types_depend_on_no_solution_namespace_except_the_shared_kernel, NFR_1_domain_assemblies_reference_no_solution_assembly_except_the_shared_kernel, NFR_1_domain_types_do_not_depend_on_ef_core_asp_net_core_or_system_text_json, NFR_1_domain_assemblies_reference_no_ef_core_asp_net_core_or_system_text_json |
| NFR-2 | P1-03 | `ContextIsolationTests`: NFR_2_context_types_do_not_depend_on_another_contexts_domain_application_or_infrastructure, NFR_2_context_assemblies_reference_no_other_contexts_domain_application_infrastructure_or_catalog, NFR_2_catalog_does_not_depend_on_any_contexts_domain_application_or_infrastructure, NFR_2_contracts_types_depend_only_on_building_blocks_contracts, NFR_2_contracts_assemblies_reference_only_building_blocks_contracts, NFR_2_reporting_depends_on_no_domain_application_or_infrastructure, NFR_2_dashboard_depends_on_no_module |
| NFR-3 | P3-05, P5-04 | |
| NFR-4 | P4-02, P4-04 | |
| NFR-5 | P4-03, P4-04 | |
| NFR-6 | P1-03, P9-01 | `DomainTestIsolationTests`: NFR_6_every_domain_test_project_on_disk_is_covered_by_the_rule, NFR_6_domain_test_projects_reference_no_ef_core_sqlite_asp_net_core_or_mocking_library, NFR_6_domain_test_projects_restore_no_ef_core_sqlite_asp_net_core_or_mocking_package (references only; the < 10 s timing is P9-01) |
| NFR-7 | P1-02 | `SkuTests`: NFR_7_valid_sku_is_created_trimmed_and_uppercased, NFR_7_invalid_sku_is_rejected, NFR_7_null_sku_is_rejected, NFR_7_sku_from_returns_a_sku_for_trusted_input, NFR_7_sku_from_throws_for_invalid_input, NFR_7_skus_with_the_same_normalised_value_are_equal; `QuantityTests`: NFR_7_non_negative_quantity_is_created, NFR_7_negative_quantity_is_rejected, NFR_7_zero_is_zero_and_not_positive, NFR_7_quantities_add, NFR_7_adding_beyond_int_range_throws, NFR_7_quantities_subtract, NFR_7_subtracting_below_zero_throws, NFR_7_quantities_compare, NFR_7_quantities_with_the_same_value_are_equal; `MoneyTests`: NFR_7_non_negative_amount_with_at_most_two_decimals_is_created, NFR_7_negative_amount_or_more_than_two_decimals_is_rejected, NFR_7_zero_is_zero_usd, NFR_7_money_adds, NFR_7_money_multiplies_by_quantity, NFR_7_money_with_the_same_amount_is_equal; `GeoPointTests`: NFR_7_point_within_range_is_created, NFR_7_point_out_of_range_is_rejected, NFR_7_distance_seattle_to_chicago_is_about_2795_km, NFR_7_distance_to_the_same_point_is_zero, NFR_7_points_with_the_same_coordinates_are_equal; `SharedKernelSurfaceTests`: NFR_7_value_objects_have_no_public_constructor_that_takes_arguments |
| NFR-8 | P1-02, P1-03 | `SharedKernelSurfaceTests`: NFR_8_shared_kernel_has_no_public_setters; `NoPublicSettersTests`: NFR_8_entities_and_aggregates_have_no_public_or_internal_setters, NFR_8_the_setter_rule_catches_public_internal_and_init_setters_but_allows_private_ones |
| NFR-9 | P9-01 | review |
| NFR-10 | P4-01, P4-04 | |
| NFR-11 | P4-03, P7-01 | |

## Architecture decisions (scaffold checks)

Not requirements, but stories cite them, so their tests carry `[Trait("Req", "ADR-xxxx")]`.

| ADR | Story | Tests |
| --- | --- | --- |
| ADR-0013 | P1-01 | `ProjectStructureTests`: ADR_0013_package_versions_are_managed_centrally, ADR_0013_all_projects_build_net9_with_strict_settings_from_directory_build_props; `HealthEndpointTests`: ADR_0013_health_endpoint_returns_ok; `TestProjectDiscoveryTests`: ADR_0013_test_project_is_discovered_by_the_runner (one per test project) |
| ADR-0020 | P1-01 | `ProjectStructureTests`: ADR_0020_sqlite_provider_is_referenced_by_building_blocks_infrastructure_and_never_by_inner_layers |
| ADR-0004 | P1-02 | `SharedKernelSurfaceTests`: ADR_0004_shared_kernel_exposes_only_the_agreed_primitives, ADR_0004_shared_kernel_references_only_the_base_class_library; `EntityTests`: ADR_0004_entities_with_the_same_id_are_equal, ADR_0004_entities_with_different_ids_are_not_equal, ADR_0004_entities_of_different_types_with_the_same_id_are_not_equal, ADR_0004_entity_is_not_equal_to_null; `AggregateRootTests`: ADR_0004_new_aggregate_has_no_domain_events_and_version_zero, ADR_0004_raised_domain_events_are_recorded_in_order, ADR_0004_clearing_domain_events_empties_the_list, ADR_0004_domain_events_cannot_be_modified_from_outside, ADR_0004_version_increments_through_the_internal_hook, ADR_0004_aggregates_use_identity_equality; `IntegrationEventContractTests`: ADR_0004_integration_event_interface_uses_only_primitives, ADR_0004_building_blocks_contracts_hold_only_the_integration_event_interface, ADR_0004_a_record_with_message_id_and_occurred_at_satisfies_the_interface |
| ADR-0014 | P1-02 | `ResultTests`: ADR_0014_domain_error_carries_code_and_message, ADR_0014_domain_error_without_a_code_is_a_programming_error, ADR_0014_success_result_is_success_and_has_no_error, ADR_0014_failure_result_carries_the_error, ADR_0014_domain_error_converts_implicitly_to_a_failed_result, ADR_0014_successful_typed_result_exposes_its_value, ADR_0014_reading_the_value_of_a_failed_typed_result_throws, ADR_0014_domain_error_converts_implicitly_to_a_failed_typed_result |
| ADR-0001 | P1-03 | `ApplicationLayerTests`: ADR_0001_application_types_do_not_depend_on_ef_core_or_asp_net_core, ADR_0001_application_assemblies_reference_no_ef_core_or_asp_net_core |
| ADR-0005 | P1-03 | `ContractsShapeTests`: ADR_0005_every_public_contracts_type_is_a_sealed_record, ADR_0005_every_integration_event_implements_the_integration_event_interface, ADR_0005_the_shape_rules_accept_a_sealed_record_event_and_reject_other_shapes |
