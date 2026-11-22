# Ubiquitous Language

Every term means one thing inside one context. The same word can mean something different in another context, and that is deliberate. **Class, method, property and event names must use these words exactly.** If you need a concept that is not here, add it here first (in the same change).

## Terms

| Term | Context | Definition | Code name |
| --- | --- | --- | --- |
| SKU | Shared | A stock-keeping unit identifier (uppercase letters, digits, `-`, 3–32 chars). Other contexts hold only the SKU, never the product | `Sku` (SharedKernel) |
| Money | Shared | A non-negative decimal amount in USD, 2 decimal places | `Money` (SharedKernel) |
| Geo point | Shared | Latitude/longitude pair | `GeoPoint` (SharedKernel) |
| Product | Catalog | A sellable item with name, unit price, dimensions and weight, identified by SKU | `Product` |
| Location | Inventory | A warehouse location with a code and coordinates | `Location`, `LocationId` |
| Stock item | Inventory | The count of one SKU at one location: on-hand, reserved, reservations, reorder threshold | `StockItem` |
| On-hand | Inventory | Units physically present at a location | `OnHand` |
| Reserved | Inventory | Units promised to orders but not yet shipped (sum of active reservations) | `Reserved` |
| Available | Inventory | On-hand minus reserved. The only number that may be promised | `Available` |
| Quantity | Inventory, Ordering, Fulfillment | A non-negative whole number of units | `Quantity` (SharedKernel) |
| Reservation | Inventory | A time-limited claim on stock at one stock item for one order line. Status: Active, Released, Expired, Committed | `Reservation`, `ReservationStatus` |
| Reservation lifetime | Inventory | How long an unconfirmed reservation lives (default 15 min) | `ReservationLifetime` |
| Confirm (a reservation) | Inventory | Stop a reservation from expiring because the order is paid and in fulfillment | `Confirm()` |
| Release | Inventory | End a reservation without shipping, returning units to available | `Release()` |
| Expire | Inventory | Release a reservation because its expiry time passed | `ExpireReservations(now)` |
| Commit | Inventory | Consume a reservation on shipment: on-hand and reserved both decrease | `Commit()` |
| Adjustment | Inventory | A manual signed correction to on-hand, always with a reason | `Adjust(change, reason)` |
| Receipt | Inventory | Stock arriving at a location, which increases on-hand | `Receive(quantity)` |
| Reorder threshold | Inventory | Available level at or below which low stock is detected | `ReorderThreshold` |
| Stock count | Inventory | A cycle count session at one location that reconciles counted vs expected quantity | `StockCount` |
| Variance | Inventory | Counted minus expected for one SKU in a stock count | `Variance` |
| Allocation strategy | Inventory | The rule choosing which location supplies a reservation | `IAllocationStrategy` |
| Order | Ordering | A customer's request to buy, made of order lines | `Order` |
| Order line | Ordering | One SKU, a quantity and a unit price within an order | `OrderLine`, `OrderLineId` |
| Customer reference | Ordering | Free-text identifier of who placed the order (no customer context in v1) | `CustomerReference` |
| Shipping address | Ordering | Where the order goes, including coordinates | `ShippingAddress` |
| Draft / Placed / Paid / Shipped / Cancelled | Ordering | Order statuses. Placed is the point after which lines can no longer change | `OrderStatus` |
| Product copy | Ordering | Ordering's local copy of SKU, price and active flag | `CatalogProduct` (Ordering.Domain) |
| Order fulfillment process | Ordering | The saga coordinating reserve → pay → pick → ship for one order | `OrderFulfillmentProcess` |
| Step | Ordering | The saga's current state | `FulfillmentStep` |
| Pick list | Fulfillment | Instructions for gathering reserved units from locations, one per order | `PickList` |
| Pick | Fulfillment | Taking units for one order line from one location | `Pick` |
| Shipment | Fulfillment | A packed order handed to a carrier, with a tracking number | `Shipment` |
| Dispatch | Fulfillment | Hand the shipment to the carrier | `Dispatch(carrier, trackingNumber)` |
| Payment | Billing | Billing's record of a charge for an order | `Payment` |
| Capture | Billing | Successfully charge a payment | `PaymentStatus.Captured` |
| Refund flag | Billing | A marker that a captured payment must be refunded (no money moves in v1) | `FlagForRefund()` |
| Payment gateway | Billing | Billing's port to a payment provider | `IPaymentGateway` |

## Words to avoid

- **"Item", "product", "stock"** on their own are ambiguous. Say *stock item*, *product*, *on-hand*, *available*.
- **"Allocate" and "reserve" are not synonyms.** Reserving is the Inventory act; the allocation strategy decides *where from*.
- **"Status" for the saga.** The order has a *status*; the saga has a *step*.
- **"Complete" for a pick list vs a pick.** A pick is *completed*; a pick list is *completed* only when all its picks are. The event for the list is `PickListCompleted`.
- **Generic names** like `Manager`, `Helper`, `Processor`, `Data`, `Info` in Domain projects.
