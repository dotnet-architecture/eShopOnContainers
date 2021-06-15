import {IOrderItem} from './orderItem.model';

export interface IOrder {
    city: number;
    street: string;
    state: string;
    country: number;
    zipcode: string;
    cardnumber: string;
    cardexpiration: Date;
    expiration: string;
    cardsecuritynumber: string;
    cardholdername: string;
    cardtypeid: number;
    buyer: string;
    ordernumber: string;
    total: number;
    orderItems: IOrderItem[];
}
