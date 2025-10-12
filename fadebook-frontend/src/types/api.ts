export interface AppointmentDto {
  appointmentId: number;
  status: string;
  customerId: number;
  serviceId: number;
  barberId: number;
  appointmentDate: string;
  customerName?: string;
  barberName?: string;
  serviceName?: string;
}

export interface BarberDto {
  barberId: number;
  username: string;
  name: string;
  specialty: string;
  contactInfo: string;
}

export interface CustomerDto {
  customerId: number;
  username: string;
  name: string;
  contactInfo: string;
}

export interface ServiceDto {
  serviceId: number;
  serviceName: string;
  servicePrice: number;
}

export interface AppointmentRequestDto {
  customer: CustomerDto;
  appointment: AppointmentDto;
}

export interface CreateAppointmentDto {
  status: string;
  customerId: number;
  serviceId: number;
  barberId: number;
  appointmentDate: string;
}

export interface CreateBarberDto {
  username: string;
  name: string;
  specialty: string;
  contactInfo: string;
  serviceIds: number[];
}

export interface CreateCustomerDto {
  username: string;
  name: string;
  contactInfo: string;
}

// Auth types
export interface UserDto {
  userId: number;
  username: string;
  email: string;
  name: string;
  phoneNumber?: string;
  role: string;
  createdAt: string;
  lastLoginAt?: string;
  isActive: boolean;
  barberId?: number;
  customerId?: number;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
  name: string;
  phoneNumber?: string;
  role?: string;
}

export interface LoginDto {
  usernameOrEmail: string;
  password: string;
}

export interface LoginResponseDto {
  token: string;
  user: UserDto;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}
