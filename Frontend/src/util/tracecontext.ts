import { trace } from '@opentelemetry/api';

export const generateTraceContext = (operationName: string) => {
    const tracer = trace.getTracer("snake");
    const span = tracer.startSpan(operationName);

    const spanContext = span.spanContext();
    const traceId = spanContext.traceId;
    const spanId = spanContext.spanId;
    const flags = spanContext.traceFlags;

    const traceparent = `00-${traceId}-${spanId}-${flags.toString(16).padStart(2, '0')}`;

    return {
        traceId,
        spanId,
        traceparent,
        span,
        end: () => span.end()
    };
};