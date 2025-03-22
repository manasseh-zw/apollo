export const config = {
    url: import.meta.env.VITE_SERVER_URL as string,
    googleClientId: import.meta.env.VITE_GOOGLE_CLIENT_ID as string,
    isProd: import.meta.env.MODE === "production",
    isDev: import.meta.env.MODE === "development",
  } as const;
  