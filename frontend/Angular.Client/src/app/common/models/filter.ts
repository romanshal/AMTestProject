export type SortBy = 'year' | 'count' | 'mass';
export type SortOrder = 'asc' | 'desc'

export interface MeteoriteFilters {
    yearFrom?: number;
    yearTo?: number;
    recclass?: string;
    name?: string;
    sortBy?: SortBy;
    sortOrder?: SortOrder
}