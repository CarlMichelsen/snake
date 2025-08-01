const isDevelopment = () => import.meta.env.MODE === 'development';

export const hostUrl = (): string => isDevelopment()
    ? "http://localhost:5100"
    : "";