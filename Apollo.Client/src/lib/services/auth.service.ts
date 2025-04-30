import type { User } from "../types/user";
import { apiRequest, type ApiResponse } from "../utils/api";

export async function googleSignup(
	accessToken: string,
): Promise<ApiResponse<User>> {
	return apiRequest<User>("/api/auth/google-signup", {
		method: "POST",
		body: JSON.stringify({ accessToken }),
	});
}

export async function emailSignup(data: {
	email: string;
	password: string;
	username: string;
}): Promise<ApiResponse<User>> {
	return apiRequest<User>("/api/auth/signup", {
		method: "POST",
		body: JSON.stringify(data),
	});
}

export async function getSignedInUser(): Promise<ApiResponse<User>> {
	return apiRequest<User>("/api/auth/me");
}

export async function signIn(data: {
	userIdentifier: string;
	password: string;
}): Promise<ApiResponse<User>> {
	return apiRequest<User>("/api/auth/signin", {
		method: "POST",
		body: JSON.stringify(data),
	});
}

export async function signOut(): Promise<ApiResponse<null>> {
	return apiRequest<null>("/api/auth/signout", {
		method: "POST",
	});
}
