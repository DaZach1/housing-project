

export interface HouseDto {
  id: number;
  number: number;
  street: string;
  city: string;
  country: string;
  postalCode: string;
  apartments: ApartmentDto[];
}

export interface ApartmentDto {
  id: number;
  number: number;
  floor: number;
  roomCount: number;
  population: number;
  totalArea: number;
  livingArea: number;
  houseId: number;
  houseLink: string;
  residents: ResidentDto[];
}

export interface ResidentDto {
  id: number;
  firstName: string;
  lastName: string;
  personalCode: string;
  dateOfBirth: Date;
  phone: string;
  email: string;
  password: string;
  apartmentId: number;
  apartmentLink: string;
  apartmentNumber?: number;
  isOwner: boolean;
  userId: string;
}
