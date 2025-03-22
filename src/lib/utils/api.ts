import { config } from "../../../config";

export type ApiResponse<T = null> = {
  data: T | null;
  success: boolean;
  errors: string[];
};

export const apiRequest = async <T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<ApiResponse<T>> => {
  try {
    const url = `${config.url}${endpoint}`;
    const response = await fetch(url, {
      ...options,
      credentials: "include", // Important for cookies
      headers: {
        "Content-Type": "application/json",
        ...options.headers,
      },
    });

    const data = await response.json();

    if (!response.ok) {
      // Handle HTTP errors
      return {
        success: false,
        data: null,
        errors: [data],
      };
    }
    return {
      success: true,
      data: data,
      errors: [],
    };
  } catch (error) {
    return {
      success: false,
      data: null,
      errors: [(error as Error).message || "API request failed"],
    };
  }
};
