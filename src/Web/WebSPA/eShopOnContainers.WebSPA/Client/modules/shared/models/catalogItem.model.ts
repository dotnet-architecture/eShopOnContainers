export interface ICatalogItem {
    id: string;
    name: string;
    description: string;
    price: number;
    pictureUri: string;
    catalogBrandId: number;
    catalogBrand: string;
    catalogTypeId: number;
    catalogType: string;
    units: number;
}
