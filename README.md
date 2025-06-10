Repositorio principal para los microservicios del Laboratorio.
para que funcione correctamente por favor crear primero el docker con:
docker compose up -d
luego ingresar a dicha instancia con:
docker exec -it cassandra cqlsh

y crear todo esto:
CREATE KEYSPACE IF NOT EXISTS laboratorio
WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};
USE laboratorio;


CREATE TABLE IF NOT EXISTS test_types (
    test_type_id int PRIMARY KEY,
    name text,
    description text,
    created_at timestamp,
    updated_at timestamp
);


-- 1. FieldsService
CREATE TABLE IF NOT EXISTS fields (
    test_type_id int,
    field_id int,
    field_name text,
    reference_range text,
    unit text,
    created_at timestamp,
    updated_at timestamp,
    PRIMARY KEY (test_type_id, field_id)
);


-- 2. ResultsService
CREATE TABLE IF NOT EXISTS results (
    result_id int PRIMARY KEY,
    order_id int,
    patient_id int,
    test_type_id int,
    status text,
    created_at timestamp,
    updated_at timestamp
);

-- 3. ResultFieldsService

CREATE TABLE IF NOT EXISTS result_fields (
    result_field_id int PRIMARY KEY,
    result_id int,
    test_type_id int,
    field_id int,
    value text,
    comment text,
    created_at timestamp,
    updated_at timestamp
);


CREATE TABLE IF NOT EXISTS patients (
    patient_id int PRIMARY KEY,
    first_name text,
    last_name text,
    date_of_birth date,
    gender text,
    email text,
    phone text,
    doctor_id int,
    created_at timestamp,
    updated_at timestamp
);

CREATE TABLE IF NOT EXISTS orders_by_doctor (
    doctor_id int,
    order_id int,
    patient_id int,
    test_type_id int,
    order_date timestamp,
    status text,
    notes text,
    PRIMARY KEY (doctor_id, order_id)
);
CREATE TABLE IF NOT EXISTS orders_by_patient (
    patient_id int,
    order_id int,
    doctor_id int,
    test_type_id int,
    order_date timestamp,
    status text,
    notes text,
    PRIMARY KEY (patient_id, order_id)
);
CREATE TABLE IF NOT EXISTS orders_by_test_type (
    test_type_id int,
    order_id int,
    doctor_id int,
    patient_id int,
    order_date timestamp,
    status text,
    notes text,
    PRIMARY KEY (test_type_id, order_id)
);
CREATE TABLE IF NOT EXISTS orders_by_patient_and_test_type (
    patient_id int,
    test_type_id int,
    order_id int,
    doctor_id int,
    order_date timestamp,
    status text,
    notes text,
    PRIMARY KEY ((patient_id, test_type_id), order_id)
);



CREATE INDEX IF NOT EXISTS idx_results_order_id ON results (order_id);
CREATE INDEX IF NOT EXISTS idx_patient_email ON patients (email);
CREATE INDEX IF NOT EXISTS idx_results_order_id ON results (order_id);
CREATE INDEX IF NOT EXISTS idx_result_fields_test_type_id ON result_fields (test_type_id);
CREATE INDEX IF NOT EXISTS idx_result_fields_field_id ON result_fields (field_id);


DATOS PARA CREAR

PACIENTE
  {
    "first_Name": "Carlos",
    "last_Name": "González",
    "date_Of_Birth": "1990-04-15T00:00:00",
    "gender": "Masculino",
    "email": "carlos.gonzalez@example.com",
    "phone": "593987654321",
    "doctor_Id": 123
  }

TEST TYPE
  {
    "name": "Hemograma Completo",
    "description": "Análisis sanguíneo completo"
  }

FIELD
  {
    "testTypeId": 1,
    "fieldName": "Glóbulos Rojos",
    "referenceRange": "4.5-6.0",
    "unit": "millones/µL"
  }

ORDER

  {
    "doctorId": 123,
    "patientId": 560852,
    "testTypeId": 1,
    "status": "Pendiente",
    "notes": "Solicitado por el Dr. Pérez"
  }

RESULTS

  {
    "orderId": 1624953129,
    "patientId": 560852,
    "testTypeId": 1,
    "status": "Pendiente"
  }


RESULT FIELD

  {
    "resultId": 1,
    "testTypeId": 1,
    "fieldId": 1,
    "value": "15",
    "comment": "todo mal"
  }






