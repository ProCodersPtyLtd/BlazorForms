CREATE TABLE public.flow_run
(
    id integer,
    flow_json jsonb,
    CONSTRAINT flow_id UNIQUE (id)
        INCLUDE(id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.flow_run
    OWNER to BlazorForms;
	
CREATE SEQUENCE public.flow_id_sequence
    INCREMENT 1
    START 100000
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.flow_id_sequence
    OWNER TO BlazorForms;