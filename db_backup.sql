--
-- PostgreSQL database dump
--

-- Dumped from database version 11.2 (Debian 11.2-1.pgdg90+1)
-- Dumped by pg_dump version 11.2 (Debian 11.2-1.pgdg90+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: my_table; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.my_table (
    id integer,
    name character varying(45)
);


--
-- Data for Name: my_table; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.my_table (id, name) FROM stdin;
0	asdf
1	qwer
3	zxcv
\.


--
-- PostgreSQL database dump complete
--

