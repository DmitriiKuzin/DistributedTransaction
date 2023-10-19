namespace Market.Mq;

public record CheckPayment(Guid OrderId);

public record PaymentSucceed(Guid OrderId);

public record PaymentFailed(Guid OrderId);

public record AllocateItemInStorage(Guid OrderId);

public record AllocationSucceed(Guid OrderId);

public record AllocationFailed(Guid OrderId);

public record CancelStorageAllocation(Guid OrderId);

public record AllocateDeliveryTimeslot(Guid OrderId);

public record AllocationDeliveryTimeslotSucceed(Guid OrderId);

public record AllocationDeliveryTimeslotFailed(Guid OrderId);