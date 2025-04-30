import { Store } from "@tanstack/react-store";
import type { User } from "../types/user";
import type { AuthState } from "../types/auth";
import type { ResearchHistoryItem } from "../types/research";
import { getResearchHistory } from "../services/research.service";

export type ResearchHistoryState = {
	items: ResearchHistoryItem[];
	totalCount: number;
	currentPage: number;
	pageSize: number;
	hasMore: boolean;
	isLoading: boolean;
	error: string | null;
};

// Create the store with initial state
export const store = new Store({
	authState: {
		user: {} as User,
		isAuthenticated: false,
		isLoading: true,
	} as AuthState,
	researchHistory: {
		items: [],
		totalCount: 0,
		currentPage: 1,
		pageSize: 5,
		hasMore: false,
		isLoading: false,
		error: null,
	} as ResearchHistoryState,
});

// Auth actions
export const authActions = {
	setUser: (user: User) => {
		store.setState((state) => ({
			...state,
			authState: {
				...state.authState,
				user,
				isAuthenticated: true,
			},
		}));
	},

	clearUser: () => {
		store.setState((state) => ({
			...state,
			authState: {
				...state.authState,
				user: {} as User,
				isAuthenticated: false,
			},
		}));
	},

	setLoading: (isLoading: boolean) => {
		store.setState((state) => ({
			...state,
			authState: {
				...state.authState,
				isLoading,
			},
		}));
	},
};

// Research History actions
export const researchHistoryActions = {
	setLoading: (isLoading: boolean) => {
		store.setState((state) => ({
			...state,
			researchHistory: {
				...state.researchHistory,
				isLoading,
			},
		}));
	},

	setError: (error: string | null) => {
		store.setState((state) => ({
			...state,
			researchHistory: {
				...state.researchHistory,
				error,
			},
		}));
	},

	// Fetch history items (used for both initial load and pagination)
	fetchHistory: async (page = 1) => {
		store.setState((state) => ({
			...state,
			researchHistory: {
				...state.researchHistory,
				isLoading: true,
				error: null,
			},
		}));

		try {
			const response = await getResearchHistory(page);
			if (response.success && response.data) {
				store.setState((state) => ({
					...state,
					researchHistory: {
						items:
							page === 1
								? //@ts-ignore
									response.data.items
								: //@ts-ignore
									[...state.researchHistory.items, ...response.data.items],
						totalCount: response.data?.totalCount ?? 0,
						currentPage: page,
						pageSize: response.data?.pageSize ?? 0,
						hasMore: response.data?.hasMore ?? false,
						isLoading: false,
						error: null,
					},
				}));
			} else {
				throw new Error("Failed to fetch research history");
			}
		} catch (error) {
			store.setState((state) => ({
				...state,
				researchHistory: {
					...state.researchHistory,
					isLoading: false,
					error: "Failed to load research history",
				},
			}));
		}
	},

	// Add a new research item to the beginning of the list
	addResearchItem: (item: ResearchHistoryItem) => {
		store.setState((state) => ({
			...state,
			researchHistory: {
				...state.researchHistory,
				items: [item, ...state.researchHistory.items],
				totalCount: state.researchHistory.totalCount + 1,
			},
		}));
	},

	// Reset the history state (useful when logging out)
	reset: () => {
		store.setState((state) => ({
			...state,
			researchHistory: {
				items: [],
				totalCount: 0,
				currentPage: 1,
				pageSize: 5,
				hasMore: false,
				isLoading: false,
				error: null,
			},
		}));
	},
};
